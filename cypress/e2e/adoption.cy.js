/// <reference types="cypress" />

describe('Automated Adoption Flow', () => {

  beforeEach(() => {
    // Automatically log in and preserve session
    cy.session('login-session', () => {
      cy.visit('https://localhost:44336/Authentication/Login')
      cy.get('input[name="email"]').type('testuser1@example.com')
      cy.get('input[name="password"]').type('Password123')
      cy.get('button[type="submit"]').click()
      cy.url().should('not.include', 'Login')
    })
  })

  it('Completes Adoption Application Flow', () => {
    // ------------------------
    // Step 1: Application1
    // ------------------------
    cy.visit('https://localhost:44336/AdoptionApplication/Application1?petId=5')
    cy.url().should('include', '/Application1')

    cy.get('input[name="FullName"]').type('Test User')
    cy.get('input[name="Email"]').type('testuser@example.com')
    cy.get('input[name="Phone"]').type('1234567890')
    cy.get('input[name="City"]').type('Kathmandu')
    cy.get('textarea[name="Address"]').type('Lalitpur')

    cy.get('form').submit()

    // ------------------------
    // Step 2: Application2
    // ------------------------
    cy.url({ timeout: 10000 }).should('include', '/Application2')

    cy.get('h2').contains('Your living situation').should('exist')

    // Select HomeType
    cy.get('input[name="HomeType"][value="House"]').check({ force: true })

    // Select Ownership
    cy.get('input[name="Ownership"][value="Own"]').check({ force: true })

    // Optional checkboxes
    cy.get('input[name="HasYard"]').check({ force: true })
    cy.get('input[name="HasChildren"]').check({ force: true })

    cy.get('form').submit()

    // ------------------------
    // Step 3: Application3
    // ------------------------
    cy.url({ timeout: 10000 }).should('include', '/Application3')
    cy.get('h2').contains('Pet experiences & documents').should('exist')

    // Optional previous pet experience
    cy.get('textarea[name="PreviousExperience"]').type('Had 2 dogs previously.')

    // Reason to adopt
    cy.get('textarea[name="ReasonToAdopt"]').type('I want to give a loving home to this pet.')

    // File upload (make sure you have 'cypress/fixtures/test-id.png')
    cy.get('input[name="IDProof"]').attachFile('test.png')

    // Agree to terms
    cy.get('input[type="checkbox"]').check({ force: true })

    // Submit final form
    cy.get('form').submit()

    // ------------------------
    // Verify final redirect or success message
    // ------------------------
    cy.url({ timeout: 10000 }).should('eq', 'https://localhost:44336/')
  })
})