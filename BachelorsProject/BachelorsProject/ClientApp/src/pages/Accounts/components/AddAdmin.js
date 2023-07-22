import React, { useState } from "react";
import Modal from "components/Modal";
import PlusInsideCircleIcon from "components/icons/PlusInsideCircleIcon";

export default function AddAdmin({ onCreateAdmin }) {
  const [isOpen, setIsOpen] = useState(false);

  const openModal = () => setIsOpen(true);
  const closeModal = () => setIsOpen(false);

  const [email, setEmail] = useState("");

  function handleSubmit(event) {
    event.preventDefault();
    onCreateAdmin(email);
    setEmail("");
    closeModal();
  }

  return (
    <>
      <a href="javascript:void(0);" className="list-item" onClick={openModal}>
        <div className="item-container">
          <PlusInsideCircleIcon />
          <p className="file-name">Create a new admin</p>
        </div>
      </a>
      <Modal isOpen={isOpen} closeModal={closeModal}>
        <form onSubmit={handleSubmit}>
          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="email"
              type="text"
              pattern="[a-zA-Z0-9._%+-]+@oslomet\.no$"
              placeholder="Ex.: s123456@oslomet.no"
              value={email}
              onChange={({ target: { value } }) => setEmail(value)}
            />
            <label className="input-container__label" htmlFor="email">
              Email:
            </label>
          </div>
          <button className="button success-color" type="submit">
            Create Admin
          </button>
        </form>
        <button onClick={closeModal} className="button danger-color">
          Cancel
        </button>
      </Modal>
    </>
  );
}
