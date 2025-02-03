import { createContext, useContext, useState, useEffect } from "react";

const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [token, setToken] = useState(localStorage.getItem("token") || null);
    const [nomeUsuario, setNomeUsuario] = useState(localStorage.getItem("nomeUsuario") || "");
    const [permissoes, setPermissoes] = useState(() => {
        try {
            const permissoesStorage = localStorage.getItem("permissoes");
            console.log("🔍 Permissões no localStorage:", permissoesStorage);
            return permissoesStorage ? JSON.parse(permissoesStorage) : [];
        } catch (error) {
            console.error("Erro ao carregar permissões:", error);
            return [];
        }
    });

    useEffect(() => {
        const atualizarDados = () => {
            try {
                const tokenStorage = localStorage.getItem("token");
                const nomeStorage = localStorage.getItem("nomeUsuario");
                const permissoesStorage = localStorage.getItem("permissoes");

                setToken(tokenStorage);
                setNomeUsuario(nomeStorage || "");

                if (permissoesStorage) {
                    setPermissoes(JSON.parse(permissoesStorage));
                } else {
                    setPermissoes([]);
                }

                console.log("🔍 Permissões carregadas:", permissoesStorage);
            } catch (error) {
                console.error("Erro ao analisar JSON das permissões:", error);
                setPermissoes([]);
            }
        };

        atualizarDados();

        window.addEventListener("storage", atualizarDados);
        return () => window.removeEventListener("storage", atualizarDados);
    }, []);

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("nomeUsuario");
        localStorage.removeItem("permissoes");
        setToken(null);
        setNomeUsuario("");
        setPermissoes([]);
    };

    return (
        <AuthContext.Provider value={{ token, nomeUsuario, permissoes, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}
