import React from "react";
import { Link } from "react-router-dom";

export default function Unauthorized() {
  return (
    <>
      <div className="does-not-exist-container">
        <h1>You do not have permission to view this page!</h1>
        <Link to="/" className="button">
          Return home
        </Link>
      </div>
    </>
  );
}
