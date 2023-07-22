describe("Test Accounts functionalities", () => {
  beforeEach(() => {
    cy.setCookie(
      "printerspace_jwt",
      "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNjg3MjEyOTAyfQ.nXcssETd2clzyUO4EK2LDjbZGvlBFpzDzsWZFSTBoR_th03x7QlfaD0LisGWJtXfHAEYLgXne-BNRwKVjhYe9g"
    );
  });

  // limited testing due to tedious server refresh for every test iteration
  // click() required at the end of the code of line 19
  it("Delete an administrator", () => {
    cy.visit("/");
    cy.get('[href="/Accounts"]').click();
    cy.get(
      ":nth-child(1) > .list > :nth-child(2) > .item-container > .file-name"
    ).click();
    cy.get(
      ":nth-child(1) > .list > :nth-child(2) > .expanded-info > div > .button"
    );
  });

  it("Delete a user", () => {
    cy.visit("/");
    cy.get('[href="/Accounts"]').click();
    cy.get(
      ":nth-child(2) > .list > :nth-child(2) > .item-container > .file-name"
    ).click();
    cy.get(
      ":nth-child(2) > .list > :nth-child(2) > .expanded-info > div > .button"
    );
  });
});
