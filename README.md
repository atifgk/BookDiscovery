# 📚 Book Discovery App

A full-stack AI-powered book discovery system that transforms messy, unstructured text queries into accurate book recommendations using **AI intent extraction (OpenAI / Gemini)**, **Open Library API**, and a custom ranking engine built with **.NET 8**.

---

## 🚀 Features

### 🧠 AI-Powered Query Understanding
Uses an LLM (OpenAI or Gemini) to extract structured intent from natural language book queries.

### 📚 Smart Book Search
Fetches relevant books from the Open Library API based on extracted intent.

### 🧮 Intelligent Ranking Engine
Custom ranking logic in .NET 8 prioritizes the most relevant results.

### 🧾 Explainable Results
Each recommendation includes a clear reason why the book was selected.

---

## ⚡ Full Stack Architecture

- 🎨 **Frontend:** React  
- ⚙️ **Backend:** .NET 8 Web API  
- 🤖 **AI Layer:** OpenAI / Gemini (Intent Extraction)  
- 📖 **Data Source:** Open Library API  

---

## 🏗️ How It Works

1. User enters a natural language query  
2. AI extracts structured intent (title, author, genre, keywords)  
3. Backend queries Open Library API  
4. Ranking engine scores and sorts results  
5. API returns structured, explainable recommendations  

---

## 🔐 Configuration

### appsettings.json

```json
{
  "Gemini": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}