# coding: utf-8
import sys
import spacy

nlp = spacy.load("ru_core_news_sm")

def process(query):
    doc = nlp(query)
    return [token.lemma_ for token in doc if not token.is_stop]

if __name__ == "__main__":
    query = sys.argv[1]
    result = process(query)
    print("hello")
    print(",".join(result))  # строка с результатами
