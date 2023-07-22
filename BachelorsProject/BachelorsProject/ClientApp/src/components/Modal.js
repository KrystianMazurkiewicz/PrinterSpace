import React, { useRef, useEffect } from "react";

export default function Modal({ isOpen, closeModal, children }) {
  const modalRef = useRef();

  useEffect(() => {
    if (isOpen) {
      modalRef.current.showModal();
    } else {
      modalRef.current.close();
    }
  }, [isOpen]);

  const handleClickOutside = (e) => {
    const { left, right, top, bottom } =
      modalRef.current.getBoundingClientRect();
    if (
      e.clientX < left ||
      e.clientX > right ||
      e.clientY < top ||
      e.clientY > bottom
    ) {
      closeModal();
    }
  };

  return (
    <>
      <dialog ref={modalRef} onClick={handleClickOutside}>
        <div className="dialog-wrapper">{children}</div>
      </dialog>
    </>
  );
}
