import { test, expect } from '@playwright/test';

// Smoke test: loans table renders and contains at least one row.
// We mock /loans to avoid backend dependency in CI.

test.beforeEach(async ({ page }) => {
  await page.route('**/loans', async (route) => {
    return route.fulfill({
      contentType: 'application/json',
      body: JSON.stringify([
        {
          id: 1,
          amount: 1500,
          currentBalance: 500,
          applicantName: 'Maria Silva',
          status: 'active',
        },
      ]),
    });
  });
});

test('loans table shows mocked data', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('heading', { name: /loan management/i })).toBeVisible();

  // Check column headers
  await expect(page.getByRole('columnheader', { name: /loan amount/i })).toBeVisible();
  await expect(page.getByRole('columnheader', { name: /current balance/i })).toBeVisible();
  await expect(page.getByRole('columnheader', { name: /applicant/i })).toBeVisible();
  await expect(page.getByRole('columnheader', { name: /status/i })).toBeVisible();

  // Check a row includes mocked applicant
  await expect(page.getByRole('row', { name: /maria silva/i })).toBeVisible();
});
