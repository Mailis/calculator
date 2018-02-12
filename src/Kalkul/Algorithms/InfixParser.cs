using CalcOperator;
using Kalkul.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kalkul.Algorithms
{
    /**
     * uses
     * Shunting-yard algorithm
     * from
     * https://en.wikipedia.org/wiki/Shunting-yard_algorithm#The_algorithm_in_detail
     **/
    public class InfixParser
    {

        Stack<string> stack;
        Queue<string> queue;
        public string error;
        List<string> inputToInfix;
        TypeObserverManager typeMan;
        DLLmanager dllMan;

        public InfixParser(List<string> _inputToInfix, TypeObserverManager _typeMan)
        {
            this.stack = new Stack<string>();
            this.queue = new Queue<string>();
            this.error = "";
            //parsed input
            this.inputToInfix = _inputToInfix;
            this.typeMan = _typeMan;
            this.dllMan = _typeMan.dllMan;
        }

        public Queue<string> getReversedPolishNotation()
        {
            //typesList is filled in CalculationController
            Dictionary<string, IGeneralOperator> operators = typeMan.typesList;


            //loop over input
            /**
             * Read a token.
             **/
            foreach (string s in inputToInfix)
            {
                //operands
                /**
                 * If the token is a number, then push it to the output queue.
                 **/
                //naively trusts that if it is not an operator, it is numeric
                if (!operators.ContainsKey(s))
                {
                    queue.Enqueue(s);//is number
                }
                else//is operator or parenthesis
                {
                    IGeneralOperator someOperator = operators[s];
                    if (someOperator.isParenthesis)
                    {
                        if (someOperator.value == "(")
                        {
                            stack.Push(someOperator.value);
                        }
                        else if (someOperator.value == ")")
                        {
                            IGeneralOperator fromStack = operators[stack.Pop()];
                            while (!fromStack.isParenthesis && fromStack.value != "(")
                            {
                                if (stack.Count() == 0)
                                {
                                    this.error = "There are mismatched parentheses.";
                                    break;
                                }
                                queue.Enqueue(fromStack.value);
                                fromStack = operators[stack.Pop()];
                            }
                        }
                    }
                    else //no parenthesis, but operator
                    {
                        if (stack.Count() != 0)
                        {
                            IOperator someOp1 = (IOperator)someOperator;
                            try
                            {
                                IOperator someOp2 = (IOperator)operators[stack.Peek()];
                                while (stack.Count() != 0 &&
                                    ((someOp1.precedence <= someOp2.precedence && someOp1.associativity == IOperator.Associativity.left) ||
                                        someOp1.precedence < someOp2.precedence && someOp1.associativity == IOperator.Associativity.right))
                                {
                                    queue.Enqueue(stack.Pop());
                                    someOp2 = (IOperator)operators[stack.Peek()];
                                }
                            }
                            catch { }
                            stack.Push(someOp1.value);
                        }
                        else
                        {
                            stack.Push(s);
                        }
                    }
                    
                }
                

            }//END OF for

            /**
             * When there are no more tokens to read:
             * While there are still operator tokens in the stack:
             * If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses.
             * Pop the operator onto the output queue.
             **/
            if (stack.Count() != 0)
            {
                string chekcParenthesis = stack.Peek();
                if (chekcParenthesis == "(" || chekcParenthesis == ")")
                {
                    this.error += "Finally, there are mismatched parentheses.";
                }
                else
                {
                    //finally add remaining operators from stack to queue
                    while (stack.Count() > 0)
                    {
                        queue.Enqueue(stack.Pop());
                    }
                }
            }
            return queue;

        }
    }
}
