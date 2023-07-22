import Modal from "components/Modal";
import React, { useState } from "react";

export default function WatchLive({ handleGetStreamFromPrinter, printerName }) {
  const [isOpen, setIsOpen] = useState(false);

  const openModal = () => setIsOpen(true);
  const closeModal = () => setIsOpen(false);

  return (
    <>
      <button
        onClick={(e) => {
          e.stopPropagation();
          openModal();
          handleGetStreamFromPrinter(printerName);
        }}
        className="unset watch-live"
      >
        Watch Live! &#9679;
      </button>
      <Modal isOpen={isOpen} closeModal={closeModal}>
        <img
          src="http://localhost:8083/"
          className="stream"
          alt="Stream of the printing process."
        />
        <button onClick={closeModal} className="button danger-color">
          Cancel Print
        </button>
        <button onClick={closeModal} className="button danger-color">
          Close
        </button>
      </Modal>
    </>
  );
}
