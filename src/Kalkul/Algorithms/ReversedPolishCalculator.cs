using CalcOperator;
using Kalkul.Managers;
using Kalkul.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kalkul.Algorithms
{
    /**
     * uses
     * Reverse_Polish_notation algorithm
     * from
     * https://en.wikipedia.org/wiki/Reverse_Polish_notation#Postfix_algorithm
     **/
    public class ReversedPolishCalculator
    {
        Stack<double> stack;
        public string error;
        TypeObserverManager typeMan;
        DLLmanager dllMan;
        Queue<string> rpn;
        public Dictionary<string, IGeneralOperator> typesList;

        public ReversedPolishCalculator(TypeObserverManager _typeMan, Queue<string> reversePolishNotation)
        {
            this.stack = new Stack<double>();
            this.error = "";
            this.typeMan = _typeMan;
            this.dllMan = _typeMan.dllMan;
            this.typesList = _typeMan.typesList;
            this.rpn = reversePolishNotation;
        }

        /**
         * @param: reversePolishNotation: Queue<string> - elements of reversed polish notation
         * @returns: double, result of RPN calculation
         * 
         **/
        public double calculateReversedPolishNotation()
        {
            double rpnResult = -1;
            //list of registered operators
            Dictionary<string, IGeneralOperator> operators = typeMan.typesList;
            //Dictionary of registered IGeneralOperator objects (<op-value, IGeneralOperator>)
            List<AssemblyItem> assemblyItems = dllMan.assemblyItems;

            /**
             * While there are input tokens left
             **/
            while (rpn.Count() > 0)
            {
                /**
                 * Read the next token from input.
                 **/
                string opString = rpn.Dequeue();
                /**
                 * If the token is a value
                 **/
                //detect numeric value operand (not operator)
                if (!operators.ContainsKey(opString))
                {
                    /**
                     * Push it onto the stack.
                     **/
                    try
                    {
                        stack.Push(Double.Parse(opString));
                    }
                    catch
                    {
                        this.error = "Operand is not a number";
                    }

                }
                /**
                 * Otherwise, the token is an operator (operator here includes both operators and functions).
                 **/
                else
                {
                    /**
                     * It is already known that the operator takes n arguments.
                     * If there are fewer than n values on the stack
                     * (Error) The user has not input sufficient values in the expression.
                     **/
                    //create IGeneralOperator object
                    IOperator iop = null;
                    int nrOfArguments = 0;
                    try
                    {
                        iop = (IOperator)this.typesList[opString];
                    }
                    catch
                    {
                        // try anoter way
                        AssemblyItem am = dllMan.getAssemblyItemByOperatorValue(opString);
                        iop = (IOperator)dllMan.getIOperatorFromDllFilePath(am.assemblyFilePath);
                    }
                    finally
                    {
                        if (iop != null)
                        {
                            nrOfArguments = iop.numberOfArguments;
                        }
                    }

                    //if operator is still null, save error and break
                    if (iop == null)
                    {
                        this.error = "Opertor " + opString + " does not extend IGeneralOperator.";
                        break;
                    }

                    if (stack.Count() < nrOfArguments)
                    {
                        this.error = "The user has not input sufficient values in the expression.";
                        break;
                    }
                    /**
                     * Else, Pop the top n values from the stack.
                     **/
                    else
                    {
                        Stack<double> poppedNumbers = new Stack<double>();
                        try
                        {
                            for (int i = 0; i < nrOfArguments; i++)
                            {
                                poppedNumbers.Push(stack.Pop());
                            }
                            /**
                             * Evaluate the operator, with the values as arguments.
                             * Push the returned results, if any, back onto the stack.
                             */
                            double compu = iop.operate(poppedNumbers);
                            this.stack.Push(compu);

                        }
                        catch (ArgumentException)
                        {
                            this.error = "Not right number of operands";
                            break;
                        }
                        catch
                        {
                            this.error = "Operand is not numeric.";
                            break;
                        }

                    }

                }
            }//END OF while (RPNinput.Count() > 0)

            /**
             * If there is only one value in the stack
             *      That value is the result of the calculation.
             * Otherwise, there are more values in the stack
             *      (Error) The user input has too many values.
             **/
            if (stack.Count == 1)
            {
                rpnResult = stack.Pop();
            }
            else
            {
                this.error = "The user input has too many values.";
            }

            return rpnResult;
        }
    }
}
