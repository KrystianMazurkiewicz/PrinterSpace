import React from "react";
import { Link } from "react-router-dom";

export default function NotFound() {
  return (
    <>
      <div className="does-not-exist-container">
        <h1>404 - This page does not exist!</h1>
        <Link to="/" className="button">
          Return home
        </Link>
      </div>
    </>
  );
}
