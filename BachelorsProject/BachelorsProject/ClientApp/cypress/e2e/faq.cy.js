describe("Test FAQ functionalities", () => {
  beforeEach(() => {
    cy.setCookie(
      "printerspace_jwt",
      "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNjg3MjEyOTAyfQ.nXcssETd2clzyUO4EK2LDjbZGvlBFpzDzsWZFSTBoR_th03x7QlfaD0LisGWJtXfHAEYLgXne-BNRwKVjhYe9g"
    );
  });

  it("Add a Q&A", () => {
    cy.visit("/");
    cy.get('[href="/FAQ"]').click();
    cy.get(".file-name").click();
    cy.get("#question").clear();
    cy.get("#question").type("q1");
    cy.get("#answer").clear();
    cy.get("#answer").type("q2");
    cy.get('[open=""] > .dialog-wrapper > form > .button').click();
  });

  it("Edit a Q&A", () => {
    cy.visit("/");
    cy.get('[href="/FAQ"]').click();
    cy.get(":nth-child(6) > .item-container").click();
    cy.get(
      ":nth-child(6) > .expanded-info > .button-container > :nth-child(1)"
    ).click();
    cy.get(
      ':nth-child(6) > .expanded-info > .button-container > dialog > .dialog-wrapper > form > [aria-label="Edit Question"]'
    )
      .clear()
      .type("Edit");
    cy.get(
      ':nth-child(6) > .expanded-info > .button-container > dialog > .dialog-wrapper > form > [aria-label="Edit Answer"]'
    )
      .clear()
      .type("Edit");
    cy.get(
      ":nth-child(6) > .expanded-info > .button-container > dialog > .dialog-wrapper > form > .button"
    ).click();
  });

  it("Delete a Q&A", () => {
    cy.visit("/");
    cy.get('[href="/FAQ"]').click();
    cy.get(":nth-child(6) > .item-container > p").click();
    cy.get(
      ':nth-child(6) > .expanded-info > .button-container > [aria-label="Delete Q&A"]'
    ).click();
  });
});
