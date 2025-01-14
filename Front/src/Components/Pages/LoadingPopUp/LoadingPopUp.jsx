import { Modal, Spinner } from 'react-bootstrap';

const LoadingPopup = () => {
    return (
        <Modal show={true} centered>
            <Modal.Body>
                <Spinner animation="grow" variant="primary" role="status" />
                <span className="ml-2">  Carregando...</span>
            </Modal.Body>
        </Modal>
    );
};

export default LoadingPopup;
