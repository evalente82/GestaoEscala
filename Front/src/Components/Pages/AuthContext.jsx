import React, { createContext, useContext, useState } from "react";

const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [token, setToken] = useState(null);
    const [permissoes, setPermissoes] = useState([]);

    const login = (novoToken, novasPermissoes) => {
        setToken(novoToken);
        setPermissoes(novasPermissoes);
        localStorage.setItem("token", novoToken);
        localStorage.setItem("permissoes", JSON.stringify(novasPermissoes));
    };

    const logout = () => {
        setToken(null);
        setPermissoes([]);
        localStorage.removeItem("token");
        localStorage.removeItem("permissoes");
    };

    return (
        <AuthContext.Provider value={{ token, permissoes, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}
