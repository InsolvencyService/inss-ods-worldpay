using System;
using System.Configuration;
using TechTalk.SpecFlow;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using TestFramework.Hooks;
using INSS.ODS.WorldpayService.AcceptanceTest;



namespace DROTestAutomation.StepDefs
{
    [Binding]
    public class CommonSteps
    {
        Context _context;
        static ExtentTest feature;
        static ExtentTest scenario;
        static ExtentReports report;
        ScenarioContext _scenarioContext;


        public CommonSteps(Context context, ScenarioContext scenarioContext)
        {
            _context = context;
            _scenarioContext = scenarioContext;
        }


        [When(@"a get request is made using (.*)")]
        public void WhenAGetRequestIsMadeUsing(string resources)
        {
            _context.GetMethod(EnvironmentData.BaseUrl, resources);
            scenario = feature.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);

        }
        [Given(@"I navigate to DRO Login page")]
        public void GivenINavigateToDROLoginPage()
        {
            _context.LoadApplication(EnvironmentData.BaseUrl);
            scenario = feature.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);

        }

        //[Given(@"I navigate to DRO Login page")]
        //public void GivenScenario()
        //{
        //    _context.LoadDROApplication();
        //    scenario = feature.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);

        //}

        [BeforeTestRun]
        public static void ReportGenerator()
        {
            var testResultReport = new ExtentHtmlReporter(AppDomain.CurrentDomain.BaseDirectory + @"TestRunReport.html");
            Console.WriteLine(testResultReport);
            testResultReport.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            report = new ExtentReports();
            report.AttachReporter(testResultReport);
        }

        [AfterTestRun]
        public static void ReportCleaner()
        {
            report.Flush();
        }

        [BeforeFeature]

        public static void BeforeFeature(FeatureContext featureContext)
        {
            feature = report.CreateTest<Feature>(featureContext.FeatureInfo.Title);
        }


        [AfterScenario]

        public void ShutdownApplication()
        {
            _context.ShutDownApplication();
        }

        [AfterStep]
        public void StepsInTheReport()
        {
            var typeOfStep = ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString();
            if (_scenarioContext == null)
            {
                if (typeOfStep.Equals("Given"))
                    scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text);
                else if (typeOfStep.Equals("When"))
                    scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text);
                else if (typeOfStep.Equals("Then"))
                    scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text);
                else if (typeOfStep.Equals("And"))
                    scenario.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text);
            }
            if (_scenarioContext.TestError != null)
            {
                if (typeOfStep.Equals("Given"))
                {
                    scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text).Fail(_scenarioContext.TestError.Message);
                }
                else if (typeOfStep.Equals("When"))
                {
                    scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text).Fail(_scenarioContext.TestError.Message);
                }
                else if (typeOfStep.Equals("Then"))
                {
                    scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text).Fail(_scenarioContext.TestError.Message);
                }
                else if (typeOfStep.Equals("And"))
                {
                    scenario.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text).Fail(_scenarioContext.TestError.Message);
                }
                if (_scenarioContext.ScenarioExecutionStatus.ToString().Equals("StepDefinitionPending"))
                {
                    if (typeOfStep.Equals("Given"))
                    {
                        scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                    }
                    else if (typeOfStep.Equals("When"))
                    {
                        scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                    }
                    else if (typeOfStep.Equals("Then"))
                    {
                        scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                    }
                    else if (typeOfStep.Equals("And"))
                    {
                        scenario.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                    }
                }



            }
        }
        [AfterScenario]

        public void CloseApplicationUnderTest()
        {
            try
            {
                if (_scenarioContext.TestError != null)
                {
                    string scenarioName = _scenarioContext.ScenarioInfo.Title;
                    string directory = AppDomain.CurrentDomain.BaseDirectory + @"\Screenshots\";
                    Console.WriteLine(directory);
                    _context.TakeScreeenshotAtThePointOfTestFailure(directory, scenarioName);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                _context.ShutDownApplication();
            }
        }



    }
}
