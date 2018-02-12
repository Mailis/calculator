using System;
using System.Collections.Generic;
using System.Linq;
using Kalkul.Algorithms;
using Microsoft.AspNetCore.Mvc;
using Kalkul.Managers;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Kalkul.Controllers
{
    [Route("api/[controller]")]
    public class CalculationController : Controller
    {
        TypeObserverManager typeMan;
        private IHostingEnvironment environment;

        public CalculationController(IHostingEnvironment _environment)
        {
            this.environment = _environment;
            this.typeMan = new TypeObserverManager(environment);
        }

        // GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(string id)
        //{
        //    return "";
        //}

        /**PIPELINE:
         * @class in object TypeObserverManager: DLLmanager - register operators, 
         *      @method: registerAllOperatorAssemblies()
         * @class: InputParser - separate operators from numeric values, 
         *      @method: parseInputToInfix(string input)
         *          @param: string input
         *          @returns: List<string>
         * @class: InfixParser - create reverse polish notation (rpn), 
         *      @method: getReversedPolishNotation()
         *          @param: List<string>
         *          @returns: Queue<string>
         * @class: ReversedPolishCalculator - calculate rpn value,
         *      @method: calculateReversedPolishNotation()
         *          @param: Queue<string>
         *          @returns: double
         **/
        // GET: /Calculation?d1={1}&d2={1}
        // GET api/values/input_string
        //[HttpGet("{input_string}")]
        public IActionResult Index(string input_string)
        {
            string errorMessage = "Invalid input.";
            if (String.IsNullOrEmpty(input_string))
            {
                return Json(errorMessage);
            }
            
            this.typeMan.registerOperators();

            InputParser input_parser = new InputParser(typeMan);
            List<string> inputToInfix = input_parser.parseInputToInfix(input_string);
            if (inputToInfix.Count() > 0)
            {
                InfixParser infix_parser = new InfixParser(inputToInfix, typeMan);
                Queue<string> reversedPolishNotation = infix_parser.getReversedPolishNotation();
                if (String.IsNullOrEmpty(infix_parser.error))
                {
                    ReversedPolishCalculator rpnCalc = new ReversedPolishCalculator(typeMan, reversedPolishNotation);
                    double answer = rpnCalc.calculateReversedPolishNotation();
                    if (String.IsNullOrEmpty(rpnCalc.error))
                    {
                        return Json(answer);
                    }
                    else
                    {//error encoutered
                        return Json(rpnCalc.error);
                    }
                }
                else
                {//error encoutered
                    return Json(infix_parser.error);
                }
            }
            else
            {//error encoutered
                return Json(errorMessage);//, JsonRequestBehavior.AllowGet);
            }


        }

    }
}
