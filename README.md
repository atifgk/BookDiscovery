# 📚 Book Discovery App

A full-stack AI-powered book discovery system that transforms messy, unstructured text queries into accurate book recommendations using **AI intent extraction (Gemini)**, **Open Library API**, and a custom ranking engine built with **.NET 8** and **React**.

---

## 🚀 Features

### 🧠 AI-Powered Query Understanding
Uses an LLM (Gemini) to extract structured intent from natural language book queries.

### 📚 Smart Book Search
Fetches relevant books from the Open Library API based on extracted intent.

### 🧮 Intelligent Ranking Engine
Custom ranking logic in .NET 8 prioritizes the most relevant results.

### 🧾 Explainable Results
Each recommendation includes a clear reason why the book was selected.

---

## ⚡ Full Stack Architecture

- 🎨 Frontend: React (Vite)
- ⚙️ Backend: ASP.NET Core 8 Web API
- 🤖 AI Layer: OpenAI / Gemini (Intent Extraction)
- 📖 Data Source: Open Library API

---

## 🏗️ How It Works

1. User enters a natural language query  
2. AI extracts structured intent (title, author, genre, keywords)  
3. Backend queries Open Library API  
4. Ranking engine scores and sorts results  
5. API returns structured, explainable recommendations  

---

## 🧰 Prerequisites

- .NET SDK 8.0+
- Node.js 18+
- npm or yarn
- Visual Studio 2022 or VS Code

---

## 🖥️ Backend Setup (.NET 8 Web API)

### Configure appsettings.json
{
  "Gemini": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}

### Run backend
dotnet restore  
dotnet build  
dotnet run  

Backend runs on:
https://localhost:5001  
http://localhost:5000  

---

## 🌐 Frontend Setup (React)

### Go to frontend folder
cd ../frontend  

### Install dependencies
npm install  

### Create .env file

VITE_API_BASE_URL=https://localhost:5001  

(If using CRA instead)
REACT_APP_API_BASE_URL=https://localhost:5001  

### Run frontend
npm run dev  

(If CRA)
npm start  

Frontend runs on:
Vite: http://localhost:5173  
CRA: http://localhost:3000  

---

## 🔗 API Flow

Frontend → ASP.NET Core Web API → AI (OpenAI/Gemini) → Open Library API → Ranking Engine → Response  

---

## 🚀 Deployment

### Backend Deployment
dotnet publish -c Release -o ./publish  

Deploy using:
- IIS (Windows Server)
- Azure App Service
- Docker

Set environment variables in production:
- Gemini:ApiKey
- OpenAI:ApiKey

---

### Frontend Deployment

npm run build  

Deploy build folder to:
- Vercel (recommended)
- Netlify
- Azure Static Web Apps
- Nginx / IIS static hosting


## 📦 Tech Stack

- React (Vite / CRA)
- ASP.NET Core 8 Web API
- OpenAI API / Gemini API
- Open Library API
- RESTful Architecture

---

## 🧠 Summary

This project demonstrates an end-to-end AI-powered book recommendation system combining modern frontend, scalable backend, and LLM-based intent extraction with intelligent ranking logic.