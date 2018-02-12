using CalcOperator;
using Kalkul.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kalkul.Algorithms
{
    /**
     * @inputItems - List<string> where operands and operators are collected
     * InputParser separates
     * operators and numeric values(including decimals).
     * Also removes whitespaces.
     **/
    public class InputParser
    {
        TypeObserverManager typeMan;

        public InputParser(TypeObserverManager _typeMan)
        {
            this.typeMan = _typeMan;
        }

        /**
          * @parameter input - string, comes from user input
          * from calculator app
          **/
        public List<string> parseInputToInfix(string input)
        {
            List<string> inputItems = new List<string>();
            //list of registered operators
            //operators are already registered in CalculationController
            Dictionary<string, IGeneralOperator> operatorKeyValue = this.typeMan.typesList;
            //sort opereators' by their value length
            string[] operators = getSortedOperators(operatorKeyValue);

            //remove white spaces from input expression
            string expression = Regex.Replace(input, @"\s+", "");
            //helps to collect operands
            StringBuilder sb = new StringBuilder();
            //for detecting consequtive operators
            string prevItem = "";
            //loop over input
            for (int i = 0; i < expression.Length; i++)
            {
                OpItem isOpera = isOperator(i, expression, operators);
                //operators
                if (isOpera != null)//an operator is encountered
                {
                    /////////////////////////////////
                    //OPERATOR ...
                    string currentOperator = isOpera.opValue;

                    /////////////////////////////////
                    //OPERAND
                    //we encoutered an operator, so add collected operands to stack
                    if (sb.Length > 0)//dont add empty sb
                    {
                        //store operand
                        prevItem = sb.ToString();
                        inputItems.Add(prevItem);
                        //init new operand collector
                        sb = new StringBuilder();
                    }
                    else
                    {
                        //if previous item was also operator, add operand 0
                        //but allow multiple parenthesis
                        if (currentOperator != "(" && currentOperator != ")")
                            if (prevItem != "(" && prevItem != ")")
                                inputItems.Add("0");
                    }

                    /////////////////////////////////
                    //... OPERATOR continues

                    //add operator to the list 
                    inputItems.Add(currentOperator);
                    //increment index i to continue looping after current operator
                    i = isOpera.index;

                    //store operator
                    prevItem = currentOperator;

                }
                //collect operands into stringbuilder sb
                else
                {
                    sb.Append(expression[i]);
                }
                
            }

            //remaining operand at the end of an input string
            if (sb.Length > 0)
            {
                inputItems.Add(sb.ToString());
            }

            return inputItems;
        }



        

        private static OpItem isOperator(int i, string expression, string[] operators)
        {
            OpItem op_item = null;

            string substrOfExpression = expression.Substring(i);
            foreach (string opItemValue in operators)
            {
                if (substrOfExpression.StartsWith(opItemValue))
                {
                    op_item = new OpItem(i += opItemValue.Length - 1, opItemValue);
                    break;
                }
            }

            return op_item;
        }


        
        private string[] getSortedOperators(Dictionary<string, IGeneralOperator> operatorKeyValue)
        {
            List<string> strList = new List<string>();
            foreach(string key in operatorKeyValue.Keys)
            {
                strList.Add(key);
            }
            //put longer operators to the beginning of an array
            strList.Sort((x, y) => y.Length.CompareTo(x.Length));
            return strList.ToArray();
        }

    }



    internal class OpItem
    {
        public string opValue { get; set; }
        public int index { get; set; }

        public OpItem(int _index, string _opValue)
        {
            this.index = _index;
            this.opValue = _opValue;
        }
    }


}
