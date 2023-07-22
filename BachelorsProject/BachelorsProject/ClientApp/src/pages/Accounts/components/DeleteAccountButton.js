import React from "react";

export default function DeleteAccountButton({ username, onDelete, label }) {
  return (
    <>
      <button
        onClick={() => {
          if (
            window.confirm(
              "Are you sure you want to permanently delete this account?"
            )
          ) {
            onDelete(username);
          }
        }}
        className="button danger-color"
      >
        {label}
      </button>
    </>
  );
}
