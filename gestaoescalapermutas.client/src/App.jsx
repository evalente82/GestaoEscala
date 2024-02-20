import './App.css';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Footer } from "./Components/Menu/NavBar"
import { Home } from './Components/Pages/Home';
import { Departamento } from './Components/Pages/Departamento/Departamento';
//import { Postos } from './Components/Pages/PostoTrabalho/PostoTrabalho';
function App() {
    return (
        <>
            
            <BrowserRouter>
                <Routes>
                    <Route path="/*" element={<Home />} />
                    <Route path="/home" element={<Home />} />
                    {/*<Route path="/postos" element={<Postos />} />*/}
                    <Route path="/departamento" element={<Departamento/>} />
                    {/*<Route path="/funcionarios" element={<Funcionarios />} />*/}
                    {/*<Route path="/" element={<Login />} />*/}
                    {/*<Route path="/escalas" element={<Escalas />} />*/}
                    {/*<Route path='/exibicao' element={<ExibicaoDaEscala />} />*/}
                    {/*<Route path="/permutas" element={<Board />} />*/}
                    {/*<Route path="/board/:idEscala" element={<Board2 />} />*/}
                    {/*<Route path="/exibicao/:idEscala" element={<Alteracao />} />*/}
                    {/*<Route path="/EditarEscalaSelecionada" element={<EditarEscalaSelecionada />} />*/}
                    {/*<Route path="/escalavigente" element={<EscalaVigente />} />*/}
                    <Route path="/departamento" element={<Departamento />} />
                    
                </Routes>
                <Footer />
            </BrowserRouter>
        </>
    );
}

export default App;