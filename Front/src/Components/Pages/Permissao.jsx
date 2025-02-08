import { useAuth } from "../Pages/AuthContext";

export default function Permissao({ permissoesNecessarias, children }) {
    const { permissoes } = useAuth();

    const temPermissao = permissoesNecessarias.some(p => permissoes.includes(p));

    return temPermissao ? children : null;
}
