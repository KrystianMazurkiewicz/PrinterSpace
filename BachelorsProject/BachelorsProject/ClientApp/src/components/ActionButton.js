import React from "react";

export default function ActionButton({
  confirmationMessage,
  onConfirm,
  label,
  classes,
  ariaLabel,
}) {
  function handleClick() {
    if (window.confirm(confirmationMessage)) onConfirm();
  }

  return (
    <button
      onClick={handleClick}
      className={`button ${classes}`}
      aria-label={ariaLabel}
    >
      {label}
    </button>
  );
}
