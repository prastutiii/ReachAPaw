describe('Pet Listing Feature', () => {

  it('Visits the Adopt page and sees pets', () => {
    cy.visit('https://localhost:44336/Adopt/Adopt')

    // Check page title
    cy.contains('Find Your').should('exist')

    // Check that at least one pet card exists
    cy.get('.pet-card').its('length').should('be.gte', 1)
  })


  it('Navigates to a pet detail page', () => {
    cy.visit('https://localhost:44336/Adopt/Adopt')

    // Click the first pet card
    cy.get('.pet-card').first().click()

    // Verify it goes to pet view page
    cy.url().should('include', '/PetView/PetView/')

    // Check pet name is visible
    cy.get('.info-main h1').should('exist')
  })

})