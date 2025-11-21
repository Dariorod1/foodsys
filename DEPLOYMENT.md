# 🍽️ Sistema de Cafetería - Guía de Despliegue

## 📋 Configuración Automática

El sistema está configurado para **detectar automáticamente** el entorno:

### 🏠 **Desarrollo Local (Tu PC)**
- **Base de datos**: SQL Server Express
- **Backend**: `http://localhost:5000`
- **Frontend**: `http://localhost:5173`

### ☁️ **Producción (Railway)**
- **Base de datos**: PostgreSQL (automático)
- **Backend**: Railway detecta automáticamente
- **Frontend**: Vercel con URL de producción

---

## 🚀 **Pasos para Subir a Railway**

### **1. Preparación Inicial**
```bash
# Instalar dependencia de PostgreSQL
cd backend/CafeteriaApi
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
```

### **2. Crear cuenta y proyecto en Railway**
1. Ir a https://railway.app
2. **Sign up** con GitHub
3. **New Project** → **Deploy from GitHub repo**
4. Seleccionar tu repositorio `foodsys`

### **3. Configurar el Backend**
1. **Root Directory**: `backend/CafeteriaApi`
2. **Start Command**: `dotnet run --urls=http://0.0.0.0:$PORT`

### **4. Agregar Base de Datos PostgreSQL**
1. En Railway dashboard → **"New Service"**
2. **Database** → **PostgreSQL**
3. Railway genera automáticamente la `DATABASE_URL`

### **5. Variables de Entorno en Railway**
```env
# Railway las configura automáticamente:
DATABASE_URL=postgresql://[generado automáticamente]
PORT=[asignado automáticamente]
ASPNETCORE_ENVIRONMENT=Production

# Tú agregas estas:
JWT_SECRET=tu-clave-super-secreta-aqui-cambiar
JWT_ISSUER=CafeteriaApi
JWT_AUDIENCE=CafeteriaApp
FRONTEND_URL=https://tu-frontend.vercel.app
```

### **6. Desplegar Frontend en Vercel**
```bash
cd frontend
npm install -g vercel
vercel login
vercel --prod
```

**Variables de entorno en Vercel:**
```env
VITE_API_URL=https://tu-backend.railway.app/api
```

---

## 🔧 **Actualizar URLs después del despliegue**

### **Paso 1: Obtener URLs**
- **Backend Railway**: `https://cafeteria-backend-production-xxxx.railway.app`
- **Frontend Vercel**: `https://cafeteria-frontend-xxxx.vercel.app`

### **Paso 2: Actualizar variables de entorno**

**En Railway (Backend):**
```env
FRONTEND_URL=https://cafeteria-frontend-xxxx.vercel.app
```

**En Vercel (Frontend):**
```env
VITE_API_URL=https://cafeteria-backend-production-xxxx.railway.app/api
```

### **Paso 3: Redeploy**
- Railway se actualiza automáticamente
- Vercel: `vercel --prod` de nuevo

---

## 🧪 **Probar que funciona**

1. **Frontend**: Acceder a tu URL de Vercel
2. **Login**: `admin@cafeteria.com` / `Admin123!`
3. **Crear pedido**: Probar funcionalidad completa
4. **Caja**: Abrir/cerrar caja
5. **Compartir**: Enviar URL a tu novia 💕

---

## 💰 **Costos**
- **Railway**: $0/mes (plan gratuito)
- **Vercel**: $0/mes (plan gratuito)
- **Total**: **$0/mes**

---

## 🔄 **Para seguir desarrollando localmente**

El sistema detecta automáticamente que estás en desarrollo y usa:
- SQL Server Express (tu configuración actual)
- `http://localhost:5000` para la API

**¡No tienes que cambiar nada para seguir trabajando local!** ✨

---

## 🆘 **Problemas Comunes**

### **Error de CORS**
- Verificar `FRONTEND_URL` en Railway
- Verificar `VITE_API_URL` en Vercel

### **Error de Base de Datos**
- Railway genera `DATABASE_URL` automáticamente
- No toques la connection string manualmente

### **Error 500**
- Verificar logs en Railway dashboard
- Verificar `JWT_SECRET` está configurado

---

## 📞 **URLs Finales**
```
Frontend (para tu novia): https://tu-app.vercel.app
Admin: admin@cafeteria.com / Admin123!
API Docs: https://tu-backend.railway.app/swagger
```

¡Listo para usar! 🎉