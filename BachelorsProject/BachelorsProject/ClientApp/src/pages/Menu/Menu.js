import React, { useEffect, useRef } from "react";
import { roleByToken } from "data/constants";
import Logo from "./components/Logo";
import { Link } from "react-router-dom";
import { ROLES } from "data/constants";

export default function Menu() {
  const menu = useRef();

  useEffect(() => {
    const menuItems = Array.from(document.getElementsByClassName("menu-item"));
    const handleMenuItemMouseOver = (event) => {
      const index = menuItems.indexOf(event.currentTarget);
      menu.current.dataset.activeIndex = index;
    };

    menuItems.forEach((item) => {
      item.addEventListener("mouseover", handleMenuItemMouseOver);
    });

    return () => {
      menuItems.forEach((item) => {
        item.removeEventListener("mouseover", handleMenuItemMouseOver);
      });
    };
  }, []);

  return (
    <>
      <Logo />
      <header id="menu" ref={menu}>
        <nav id="menu-items">
          {roleByToken === ROLES.admin ? <AdminLinks /> : <UserLinks />}
        </nav>
        <div id="menu-background-pattern"></div>
        <div id="menu-background-image"></div>
      </header>
    </>
  );
}

function AdminLinks() {
  return (
    <>
      <Link className="menu-item" to="/Printers">
        Printers
      </Link>
      <Link className="menu-item" to="/Queue">
        Queue
      </Link>
      <Link className="menu-item" to="/Accounts">
        Accounts
      </Link>
      <Link className="menu-item" to="/FAQ">
        FAQ
      </Link>
    </>
  );
}

function UserLinks() {
  return (
    <>
      <Link className="menu-item" to="/Start_Printing">
        Start Printing
      </Link>
      <Link className="menu-item" to="/My_Prints">
        My Prints
      </Link>
      <Link className="menu-item" to="/FAQ">
        FAQ
      </Link>
      <a
        className="menu-item"
        href="https://www.oslomet.no/om/tkd/makerspace"
        target="_blank"
        rel="noreferrer"
      >
        Contact Us
      </a>
    </>
  );
}
