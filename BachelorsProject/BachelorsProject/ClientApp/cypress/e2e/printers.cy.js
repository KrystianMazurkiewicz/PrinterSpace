describe("Test Printers functionalities", () => {
  beforeEach(() => {
    cy.setCookie(
      "printerspace_jwt",
      "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNjg3MjEyOTAyfQ.nXcssETd2clzyUO4EK2LDjbZGvlBFpzDzsWZFSTBoR_th03x7QlfaD0LisGWJtXfHAEYLgXne-BNRwKVjhYe9g"
    );
  });

  it("Add printer1", () => {
    cy.visit("/");
    cy.get('[href="/Printers"]').click();
    cy.get(".item-container").find(".file-name").last().click();
    cy.get("#printerName").clear();
    cy.get("#printerName").type("p1");
    cy.get(":nth-child(2) > #ipAddress").clear();
    cy.get(":nth-child(2) > #ipAddress").type("192.2.2.1");
    cy.get(":nth-child(3) > #apiKey").clear();
    cy.get(":nth-child(3) > #apiKey").type("apikey");
    cy.get(":nth-child(4) > #apiSecret").clear();
    cy.get(":nth-child(4) > #apiSecret").type("apisecret");
    cy.get('[open=""] > .dialog-wrapper > form > .button').click();
  });

  it("Edit printer1", () => {
    cy.visit("/");
    cy.get('[href="/Printers"]').click();
    cy.get(":nth-child(2) > .item-container > .file-name").click();
    cy.get(
      ":nth-child(2) > .expanded-info > .button-container > :nth-child(1)"
    ).click();
    cy.get(
      '[open=""] > .dialog-wrapper > form > :nth-child(1) > #ipAddress'
    ).clear();
    cy.get(
      '[open=""] > .dialog-wrapper > form > :nth-child(1) > #ipAddress'
    ).type("192.2.2.2");
    cy.get(
      '[open=""] > .dialog-wrapper > form > :nth-child(2) > #apiKey'
    ).clear();
    cy.get('[open=""] > .dialog-wrapper > form > :nth-child(2) > #apiKey').type(
      "apikey1"
    );
    cy.get(
      '[open=""] > .dialog-wrapper > form > :nth-child(3) > #apiSecret'
    ).clear();
    cy.get(
      '[open=""] > .dialog-wrapper > form > :nth-child(3) > #apiSecret'
    ).type("apisecret1");
    cy.get('[open=""] > .dialog-wrapper > form > .button').click();
  });

  it("Lock printer1", () => {
    cy.visit("/");
    cy.get('[href="/Printers"]').click();
    cy.get(":nth-child(2) > .item-container > .file-name").click();
    cy.get(
      ':nth-child(2) > .expanded-info > .button-container > [aria-label="Change Lock Status"]'
    ).click();
  });

  it("Show prints history for printer1", () => {
    cy.visit("/");
    cy.get('[href="/Printers"]').click();
    cy.get(":nth-child(2) > .item-container > .file-name").click();
    cy.get(
      ":nth-child(2) > .expanded-info > .button-container > .read-color"
    ).click();
    cy.get('[open=""] > .dialog-wrapper > .button').click();
  });

  it("Delete printer1", () => {
    cy.visit("/");
    cy.get('[href="/Printers"]').click();
    cy.get(".list > :nth-child(2)").click();
    cy.get(
      ':nth-child(2) > .expanded-info > .button-container > [aria-label="Delete printer"]'
    ).click();
  });
});
