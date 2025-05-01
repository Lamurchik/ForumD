using HotChocolate.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Language;
using Forum.Model.DB;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.ComponentModel;

namespace Forum.Model
{
    public class GraphQLCacheMiddleware
    {

        private readonly FieldDelegate _next;
        private readonly IDistributedCache _cache;

        public GraphQLCacheMiddleware(FieldDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            // Проверяем, является ли запрос Query
            if (!context.Operation.Definition.Operation.Equals(HotChocolate.Language.OperationType.Query))
            {
                await _next(context);
                return;
            }

            // Создаём уникальный ключ кэша на основе имени поля и аргументов
            string cacheKey = GenerateCacheKey(context);

            // Проверяем, есть ли данные в Redis
            string? cachedResult = await _cache.GetStringAsync(cacheKey);
            if (cachedResult != null)
            {
                // Определяем тип результата
                

                    var resultType = context.Selection.Field.RuntimeType;

                    // Десериализуем в нужный тип
                    var result = JsonSerializer.Deserialize(cachedResult, resultType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    context.Result = result;
                    Console.WriteLine("отработал redis");
                    return;
                
                
            }

            // Если в кэше нет данных, выполняем следующий middleware
            await _next(context);

            // Если результат успешный, кэшируем его
            if (context.Result != null && context.Result is IQueryable queryable)
            {
                // Получаем тип `T` из `IQueryable<T>`
                Type entityType = queryable.ElementType;

                // Динамически вызываем `ToListAsync<T>()`
                var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync))!
                    .MakeGenericMethod(entityType);

                // Выполняем `ToListAsync()` асинхронно
                var task = (Task)toListAsyncMethod.Invoke(null, new object[] { queryable, CancellationToken.None })!;
                await task.ConfigureAwait(false);

                // Получаем результат `List<T>`
                var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result))!;
                var resultList = resultProperty.GetValue(task);

                // Сериализуем и кэшируем
                var resultJson = JsonSerializer.Serialize(resultList);
                context.Result = resultList;
                await _cache.SetStringAsync(cacheKey, resultJson, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                });
            }
            //else if(context.Result != null) //для другого типа 
            //{
            //    var resultType = context.Result.GetType();

            //    // Проверим, не является ли это Connection<T>
            //    if (resultType.IsGenericType && resultType.GetGenericTypeDefinition().Name.StartsWith("Connection"))
            //    {
            //        var resultJson = JsonSerializer.Serialize(context.Result);
            //        await _cache.SetStringAsync(cacheKey, resultJson, new DistributedCacheEntryOptions
            //        {
            //            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            //        });
            //    }
            //}
        }
       
        private string GenerateCacheKey(IMiddlewareContext context)
        {
            var sb = new StringBuilder();
            sb.Append(context.Selection.Field.Name);

            // Добавляем аргументы запроса
            foreach (var argument in context.Selection.Arguments)
            {
                sb.Append($"|{argument.Name}:{JsonSerializer.Serialize(context.ArgumentValue<object?>(argument.Name))}");
            }

            // Получаем список выбранных полей (только верхнего уровня)
            var selectedFields = context.Selection.SelectionSet?.Selections
                .OfType<FieldNode>()
                .Select(field => field.Alias?.Value ?? field.Name.Value)
                .OrderBy(name => name)
                .ToList();

            if (selectedFields != null && selectedFields.Any())
            {
                sb.Append("|Fields:" + string.Join(",", selectedFields));
            }

            return $"graphql:{ComputeSha256Hash(sb.ToString())}";
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return Convert.ToHexString(bytes); // Преобразуем в HEX строку
            }
        }

    }
}
