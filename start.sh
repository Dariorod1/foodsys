#!/bin/bash
cd backend/CafeteriaApi
dotnet restore
dotnet run --urls=http://0.0.0.0:$PORT