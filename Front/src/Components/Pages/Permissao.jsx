import { useAuth } from "./AuthContext";

export default function Permissao({ permissoesNecessarias, children, requireAll = false }) {
    const { permissoes } = useAuth();

    if (!permissoes || permissoes.length === 0) {
        return null;
    }

    const temPermissao = requireAll
        ? permissoesNecessarias.every(p => permissoes.includes(p))
        : permissoesNecessarias.some(p => permissoes.includes(p));

    return temPermissao ? children : null;
}
