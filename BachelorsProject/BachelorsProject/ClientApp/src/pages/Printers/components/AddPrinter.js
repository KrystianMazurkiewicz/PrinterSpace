import React, { useState } from "react";
import Modal from "components/Modal";
import PlusInsideCircleIcon from "components/icons/PlusInsideCircleIcon";

export default function AddPrinter({ onCreate }) {
  const [isOpen, setIsOpen] = useState(false);

  const openModal = () => setIsOpen(true);
  const closeModal = () => setIsOpen(false);

  const [printerName, setPrinterName] = useState("");
  const [ipAddress, setIpAddress] = useState("");
  const [apiKey, setApiKey] = useState("");
  const [apiSecret, setApiSecret] = useState("");

  function handleSubmit(event) {
    event.preventDefault();
    onCreate(printerName, ipAddress, apiKey, apiSecret);
    setPrinterName("");
    setIpAddress("");
    setApiKey("");
    setApiSecret("");
    closeModal();
  }

  return (
    <>
      <a href="javascript:void(0);" className="list-item" onClick={openModal}>
        <div className="item-container">
          <PlusInsideCircleIcon />
          <p className="file-name">Add a new printer</p>
        </div>
      </a>
      <Modal isOpen={isOpen} closeModal={closeModal}>
        <form onSubmit={handleSubmit}>
          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="printerName"
              type="text"
              value={printerName}
              onChange={({ target: { value } }) => setPrinterName(value)}
            />
            <label className="input-container__label" htmlFor="printerName">
              Printer Name:
            </label>
          </div>

          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="ipAddress"
              type="text"
              value={ipAddress}
              pattern="\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"
              onChange={({ target: { value } }) => setIpAddress(value)}
            />
            <label className="input-container__label" htmlFor="ipAddress">
              IP Address:
            </label>
          </div>

          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="apiKey"
              type="text"
              value={apiKey}
              onChange={({ target: { value } }) => setApiKey(value)}
            />
            <label className="input-container__label" htmlFor="apiKey">
              API Key:
            </label>
          </div>

          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="apiSecret"
              type="text"
              value={apiSecret}
              onChange={({ target: { value } }) => setApiSecret(value)}
            />
            <label className="input-container__label" htmlFor="apiSecret">
              Secret API Key:
            </label>
          </div>
          <button className="button success-color" type="submit">
            Add Printer
          </button>
        </form>
        <button onClick={closeModal} className="button danger-color">
          Cancel
        </button>
      </Modal>
    </>
  );
}
