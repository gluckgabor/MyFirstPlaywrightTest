using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{
    public IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;

    // OneTimeSetUp: Runs once before any test runs in the class
    [OneTimeSetUp]
    public async Task Setup()
    {
        // Initialize Playwright and launch the browser
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false, // For visual UI
            Args = new[] { "--start-maximized" } // Maximizes window
        });

        // Create a new browser context and page for each test
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    [Test]
    public async Task HasTitle()
    {
        await _page.GotoAsync("https://commitquality.com/");

        await Task.Delay(5000); // 5000 milliseconds = 5 seconds

        // Expect a title "to contain" a substring.
        await Expect(_page).ToHaveTitleAsync(new Regex("CommitQuality - Test Automation Demo"));
    }

    [Test]
    public async Task GetStartedLink()
    {
        await _page.SetViewportSizeAsync(1536, 824);
        await _page.GotoAsync("https://commitquality.com/");

        // define elements
        var AddAProductButton = _page.Locator("//a[contains(text(), 'Add a Product')]");
        var ProductNameInputBox = _page.GetByTestId("product-textbox");
        var PriceInputBox = _page.GetByTestId("price-textbox");
        var DateStockedInputBox = _page.GetByTestId("date-stocked");
        var SubmitButton = _page.GetByTestId("submit-form");

        // Wait for element to be visible before clicking
        await AddAProductButton.WaitForAsync();

        // Click the element
        await AddAProductButton.ClickAsync();

        // Wait for the element to be visible before clicking
        await ProductNameInputBox.WaitForAsync();
        await PriceInputBox.WaitForAsync();
        await DateStockedInputBox.WaitForAsync();
        await SubmitButton.WaitForAsync();

        // Add new Product
        await ProductNameInputBox.FillAsync("Toothbrush");
        await PriceInputBox.FillAsync("1000");

        DateTime currentDate = DateTime.Today;
        await DateStockedInputBox.FillAsync(currentDate.ToString("yyyy-MM-dd"));        
        await _page.Keyboard.PressAsync("Enter");

        await AddAProductButton.ClickAsync();

        await Task.Delay(5000); // 5000 milliseconds = 5 seconds
    }
}