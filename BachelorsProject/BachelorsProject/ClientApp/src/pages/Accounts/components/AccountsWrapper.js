import HomeIcon from "components/icons/HomeIcon";
import React from "react";
import { Link } from "react-router-dom";

export default function AccountsWrapper({ children }) {
  return (
    <main className={"Accounts"}>
      <div className="page-wrapper">
        <header className="list-header">
          <h1>Accounts</h1>
          <nav className="list-header__nav">
            <Link className="list-header__nav__item" to="/">
              <HomeIcon />
            </Link>
          </nav>
        </header>
        <div id="accounts-wrapper">{children}</div>
      </div>
    </main>
  );
}
