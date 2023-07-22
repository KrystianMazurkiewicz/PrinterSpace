import login from "api/read/login";
import Horizontal1 from "components/logos/Horizontal1";
import React, { useState } from "react";

export default function Login() {
  const [showSuccessMessage, setShowSuccessMessage] = useState(false);

  function submit(e, email) {
    e.preventDefault();
    setShowSuccessMessage(true);
    login(email);
  }

  return (
    <>
      <div className="login-body">
        <div className="login-body__container">
          <div className="svg-container-horizontal-1">
            <Horizontal1 />
          </div>
          {showSuccessMessage ? <SuccessMessage /> : <Form submit={submit} />}
        </div>
      </div>
    </>
  );
}

function Form({ submit }) {
  const [email, setEmail] = useState(null);

  return (
    <>
      <h1 className="login-body__title">Log in with email</h1>
      <form
        className="login-form"
        onSubmit={(e) => {
          e.preventDefault();
          submit(e, email);
        }}
      >
        <label className="login-form__label" htmlFor="email-input">
          Email address
        </label>
        <input
          className="login-form__input"
          type="email"
          id="email-input"
          name="email"
          pattern="[a-zA-Z0-9._%+-]+@oslomet\.no$"
          placeholder="Ex.: s123456@oslomet.no"
          onChange={({ target: { value } }) => setEmail(value)}
          required
        />
        <button className="login-form__submit" type="submit">
          LOGIN / REGISTER
        </button>
      </form>
    </>
  );
}

function SuccessMessage() {
  return (
    <div className="successful-login-message">
      <h1>We have sent you an email!</h1>
      <p>Check you email and use the link to log in!</p>
    </div>
  );
}
