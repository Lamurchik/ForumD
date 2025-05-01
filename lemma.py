# coding: utf-8
import sys
import json
import spacy
import io

# Принудительная установка кодировки вывода
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

# Кэширование модели
if not hasattr(spacy, 'cached_model'):
    try:
        nlp = spacy.load("ru_core_news_sm")
    except OSError:
        spacy.cli.download("ru_core_news_sm")
        nlp = spacy.load("ru_core_news_sm")
    spacy.cached_model = nlp

def process(text: str) -> list:
    doc = spacy.cached_model(text)
    return [
        token.lemma_.strip().lower()
        for token in doc
        if token.lemma_.strip() 
        and not token.is_stop
        and not token.is_punct
        and not token.is_space
    ]

if __name__ == "__main__":
    try:
        print(json.dumps(process(sys.argv[1]), ensure_ascii=False, indent=None))
    except Exception as e:
        print(json.dumps({"error": str(e)}, ensure_ascii=False))
        sys.exit(1)
