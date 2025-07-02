import { createContext, useContext, useState, useEffect } from "react";

const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [token, setToken] = useState(localStorage.getItem("token") || null);
    const [nomeUsuario, setNomeUsuario] = useState(localStorage.getItem("nomeUsuario") || "");
    const [permissoes, setPermissoes] = useState(() => {
        try {
            return JSON.parse(localStorage.getItem("permissoes")) || [];
        } catch {
            return [];
        }
    });
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const handleBackButton = () => {
            //console.log("🔄 Tentativa de voltar detectada!");
            window.location.reload(); // 🔹 Força um recarregamento completo
        };
    
        window.addEventListener("popstate", handleBackButton);
    
        return () => {
            window.removeEventListener("popstate", handleBackButton);
        };
    }, []);
    

    useEffect(() => {
        const atualizarDados = () => {
            setToken(localStorage.getItem("token"));
            setNomeUsuario(localStorage.getItem("nomeUsuario") || "");
            setPermissoes(JSON.parse(localStorage.getItem("permissoes")) || []);
            setLoading(false);
        };

        atualizarDados();
        window.addEventListener("storage", atualizarDados);
        return () => window.removeEventListener("storage", atualizarDados);
    }, []);

    const login = (token, nomeUsuario, permissoes) => {
        localStorage.setItem("token", token);
        localStorage.setItem("nomeUsuario", nomeUsuario);
        localStorage.setItem("permissoes", JSON.stringify(permissoes));

        setToken(token);
        setNomeUsuario(nomeUsuario);
        setPermissoes(permissoes);
    };

    const logout = () => {
        //console.log("🚪 Realizando logout...");
    
        localStorage.clear(); // 🔹 Remove todos os dados do usuário
    
        setToken(null);
        setNomeUsuario("");
        setPermissoes([]);
    
        // 🔹 Remove todas as entradas do histórico do navegador
        window.history.pushState(null, "", "/");
        window.history.replaceState(null, "", "/");
    
        // 🔹 Bloqueia qualquer tentativa de voltar no histórico
        window.onpopstate = function () {
            window.history.go(1);
        };
    
        // 🔹 Redireciona imediatamente para a página de login
        window.location.href = "/";
    };
    
    
    
    

    return (
        <AuthContext.Provider value={{ token, nomeUsuario, permissoes, login, logout, loading }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}
