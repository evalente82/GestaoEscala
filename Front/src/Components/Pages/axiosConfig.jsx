import axios from "axios";

const API_BASE_URL = import.meta.env.VITE_BACKEND_API;

// 🔹 Cria uma instância do Axios com configuração global
const api = axios.create({
    baseURL: API_BASE_URL
});

// 🔹 Interceptador para adicionar o token a todas as requisições
api.interceptors.request.use(config => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, error => {
    return Promise.reject(error);
});

export default api;
