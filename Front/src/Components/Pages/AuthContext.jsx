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
        localStorage.clear();
        setToken(null);
        setNomeUsuario("");
        setPermissoes([]);
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
