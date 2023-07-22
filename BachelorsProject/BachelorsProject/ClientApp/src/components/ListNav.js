import { Link } from "react-router-dom";
import HomeIcon from "./icons/HomeIcon";

export default function ListNav({ pageName, children }) {
  return (
    <main className={pageName}>
      <div className="page-wrapper">
        <header className="list-header">
          <h1>{pageName}</h1>
          <nav className="list-header__nav">
            <Link className="list-header__nav__item" to="/">
              <HomeIcon />
            </Link>
          </nav>
        </header>
        <div className="list">{children}</div>
      </div>
    </main>
  );
}
