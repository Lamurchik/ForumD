# Forum
Для работы приложения нужны следующие внешние службы:
Redis
postgresSQL
openSerch
rabbitMQ 
для запуска используется docker 
запустим каждый сервис
postgresSQL
docker run --name Data –p 5432:5432 –e POSTGRES_DB=DB –e POSTGRES_USER=Vlad –e POSTGRES_PASSWORD=123 postgres
далее нужно создать миграцию бд 
заходим в консоль разработчика и пишем:
Redis
docker run -d --name redis-stack-server -p 6379:6379 redis/redis-stack-server:latest
openserch
нужно скачать прикрепленый yml файл и открыв его в консоли ввести команду:
docker-compose up
