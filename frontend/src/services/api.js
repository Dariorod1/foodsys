import axios from 'axios';

// Configuración automática de la URL de la API
const getApiUrl = () => {
  // En desarrollo
  if (import.meta.env.DEV) {
    return 'http://localhost:5000/api';
  }
  
  // En producción, usar variable de entorno o URL por defecto
  return import.meta.env.VITE_API_URL || 'https://foodsys-production.up.railway.app/api';
};

const API_URL = getApiUrl();

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Interceptor para agregar token a las requests
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Autenticación
export const autenticacionService = {
  registro: (data) => api.post('/autenticacion/registro', data),
  login: (data) => api.post('/autenticacion/login', data),
  me: () => api.get('/autenticacion/me')
};

// Productos
export const productosService = {
  obtenerTodos: () => api.get('/productos'),
  obtenerPorId: (id) => api.get(`/productos/${id}`),
  obtenerPorCategoria: (categoria) => api.get(`/productos/categoria/${categoria}`),
  crear: (data) => api.post('/productos', data),
  actualizar: (id, data) => api.put(`/productos/${id}`, data),
  eliminar: (id) => api.delete(`/productos/${id}`)
};

// Mesas
export const mesasService = {
  obtenerTodas: () => api.get('/mesas'),
  obtenerPorId: (id) => api.get(`/mesas/${id}`),
  obtenerDisponibles: () => api.get('/mesas/disponibles/listar'),
  crear: (data) => api.post('/mesas', data),
  actualizar: (id, data) => api.put(`/mesas/${id}`, data),
  eliminar: (id) => api.delete(`/mesas/${id}`)
};

// Pedidos
export const pedidosService = {
  obtenerTodos: () => api.get('/pedidos'),
  obtenerPorId: (id) => api.get(`/pedidos/${id}`),
  obtenerPendientes: () => api.get('/pedidos/pendientes/listar'),
  obtenerPorMesa: (mesaId) => api.get(`/pedidos/mesa/${mesaId}`),
  crear: (data) => api.post('/pedidos', data),
  actualizarEstado: (id, data) => api.put(`/pedidos/${id}/estado`, data),
  eliminar: (id) => api.delete(`/pedidos/${id}`)
};

// Reportes
export const reportesService = {
  ventas: (fechaInicio, fechaFin) => 
    api.get('/reportes/ventas', { params: { fechaInicio, fechaFin } }),
  productosMasVendidos: (fechaInicio, fechaFin) => 
    api.get('/reportes/productos-mas-vendidos', { params: { fechaInicio, fechaFin } }),
  ventasPorDia: (fechaInicio, fechaFin) => 
    api.get('/reportes/ventas-por-dia', { params: { fechaInicio, fechaFin } }),
  ingresoTotal: (fechaInicio, fechaFin) => 
    api.get('/reportes/ingreso-total', { params: { fechaInicio, fechaFin } }),
  promedioVenta: (fechaInicio, fechaFin) => 
    api.get('/reportes/promedio-venta', { params: { fechaInicio, fechaFin } })
};

// Cierre de Caja
export const cierreCajaService = {
  abrirCaja: (data) => api.post('/cierrecaja/abrir', data),
  cerrarCaja: (id, data) => api.post(`/cierrecaja/${id}/cerrar`, data),
  obtenerAbierta: () => api.get('/cierrecaja/abierta'),
  obtenerPorFecha: (fecha) => api.get('/cierrecaja/por-fecha', { params: { fecha } }),
  obtenerCierresDelDia: (fecha) => api.get('/cierrecaja/por-fecha', { params: { fecha } }),
  obtenerPorId: (id) => api.get(`/cierrecaja/${id}`)
};

export default api;
