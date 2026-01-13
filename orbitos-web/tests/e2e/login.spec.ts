import { test, expect } from '@playwright/test'

test.describe('Login Page', () => {
  test('should display login page when not authenticated', async ({ page }) => {
    await page.goto('/')

    // Check for OrbitOS title
    await expect(page.getByRole('heading', { name: /OrbitOS/i })).toBeVisible()

    // Check for sign in button
    await expect(page.getByRole('button', { name: /sign in with microsoft/i })).toBeVisible()
  })

  test('should have sign in button that can be clicked', async ({ page }) => {
    await page.goto('/')

    const signInButton = page.getByRole('button', { name: /sign in with microsoft/i })
    await expect(signInButton).toBeEnabled()
  })
})

test.describe('Navigation', () => {
  test('should redirect to login when accessing protected route without auth', async ({ page }) => {
    await page.goto('/dashboard')

    // Should redirect to home/login page or show login UI
    await expect(page.getByRole('button', { name: /sign in with microsoft/i })).toBeVisible()
  })
})
