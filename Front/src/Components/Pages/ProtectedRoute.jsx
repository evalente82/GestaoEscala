import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../Pages/AuthContext"; 
import { useEffect } from "react";

const ProtectedRoute = () => {
    const { token } = useAuth();

    useEffect(() => {
        if (!token) {
            //console.log("🔐 Usuário não autenticado! Redirecionando...");
            window.location.replace("/"); // 🔹 Garante que o usuário seja redirecionado sem ver a interface
        }
    }, [token]);

    return token ? <Outlet /> : null;
};

export default ProtectedRoute;

