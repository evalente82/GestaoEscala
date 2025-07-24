import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../Pages/AuthContext";

const ProtectedRoute = () => {
    const { token } = useAuth();
    
    // O componente Navigate lida com o redirecionamento de forma declarativa e integrada ao React Router.
    // Ele não causa um refresh na página, mantendo o estado da aplicação.
    if (!token) {
        //console.log("🔐 Usuário não autenticado! Redirecionando...");
        return <Navigate to="/" replace />;
    }
 
    return <Outlet />;
};

export default ProtectedRoute;
