using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StochasticLSystem {
    private Dictionary<string, List<string>> rules = new Dictionary<string, List<string>>();
    private Dictionary<string, List<double>> productionProbabilities = new Dictionary<string, List<double>>();
    private string axiom;

    public StochasticLSystem(string axiom) {
        this.axiom = axiom;
    }

    public string GetSystem(int iterations) {
        string lSystem = axiom;

        for (int i = 0; i < iterations; i++) {
            StringBuilder next = new StringBuilder();
            foreach (char c in lSystem) {
                if (rules.ContainsKey(c.ToString())) {
                    next.Append(GetRule(c.ToString()));
                } else {
                    next.Append(c);
                }
            }

            lSystem = next.ToString();
        }

        return lSystem;
    }
    

    public void InsertRule(string production, string rule, double probability) {
        if (!rules.ContainsKey(production)) {
            rules[production] = new List<string>();
            productionProbabilities[production] = new List<double>();
        }

        rules[production].Add(rule);
        productionProbabilities[production].Add(probability);
    }

    private string GetRule(string production) {
        List<string> prodRules = rules[production];
        List<double> probabilities = productionProbabilities[production];
        Debug.Assert(prodRules.Count == probabilities.Count);

        List<double> ranges = new List<double>();  // If 3 rules with 0.25, 0.5, and 0.25 probability, then ranges will be 0.25, 0.75, 1.0 
        // Then take the random number and pick the prodRule index that is below the ranges[index] when iterating.
        double maxRandomNum = 0;
        foreach (double probability in probabilities) {
            maxRandomNum += probability;
            ranges.Add(maxRandomNum);
        }
        Debug.Assert(maxRandomNum <= 1.0f);

        double randomNum = Random.Range(0, (float)maxRandomNum);
        for (int i = 0; i < prodRules.Count; i++) {
            if (randomNum <= ranges[i]) {
                return prodRules[i];
            }
        }

        Debug.Assert(false); // Should never happen
        return null;
    }
}
