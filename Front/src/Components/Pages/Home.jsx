
import NavBar from "../Menu/NavBar";
import logo1 from "../../Components/Imagens/LogoDefesaCivil.png";
import logo2 from '../../Components/Imagens/SalvamentoMaritimo.png';
import './Home.css';
export function Home() {
    return (
        <>
            <NavBar />
            <div className='container'>
                <h2 className='text-center mb-3'>Home Page</h2>
            </div>
            <div className="image-container" style={{ display: 'flex', justifyContent: 'center' }}>
                <img className="logos-home" src={logo1} alt="Logo Defesa " />
                <img className="logos-home" style={{ width: "600px" }} src={logo2} alt="Logo Defesa civil de Maricá" />
            </div>
        </>
    );
}
