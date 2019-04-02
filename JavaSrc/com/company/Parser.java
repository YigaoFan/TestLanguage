package com.company;

import java.util.ArrayList;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * Created by LaoFan on 3/29/19.
 */
public class Parser {
    private Node headNode;
    private int netLength;
    private String netCode;
    private String name;
    private ArrayList<Token> tokenList;
    private Node currentNode; // maybe not need
    // indicate the current scope, should be parameter
//    private int leftIndex;
//    private int rightIndex;

    public static Tree parse(String code) {
        Parser p = new Parser(code);
        return new Tree(p.name, p.headNode);
    }

    private static String removeBlank(String code) {
        String dest = null;
        if (code != null) {
            Pattern p = Pattern.compile("\\s*|\t|\r|\n"); // TODO check if it remove the space, \s is space
            Matcher m = p.matcher(code);
            dest = m.replaceAll("");
        }
        return dest;
    }

    private Parser(String code) {
        netCode = Parser.removeBlank(code);
        netLength = netCode.length();
        headNode = tokenize().check().doParse(3);
    }

    private Parser tokenize() {
        tokenList = new ArrayList<Token>();
        StringBuilder currentBuffer = new StringBuilder();
        int currentLevel = 0;

        for (int i = 0; i < netLength; ++i) {
            switch (netCode.charAt(i)) {
                case '(':
                    ++currentLevel;
                    tokenList.add(new Token("(", currentLevel, TokenType.leftBracket));
                    break;
                case ')':
                    tokenList.add(new Token(")", currentLevel, TokenType.rightBracket));
                    --currentLevel;
                    break;
                case '[':
                    ++currentLevel;
                    tokenList.add(new Token("[", currentLevel, TokenType.leftSquareBracket));
                    break;
                case ']':
                    tokenList.add(new Token("]", currentLevel, TokenType.rightSquareBracket));
                    --currentLevel;
                    break;
                case ' ':
                    tokenList.add(new Token(currentBuffer.toString(), currentLevel, TokenType.word));
                    currentBuffer.delete(0, currentBuffer.length());
                    break;
                default:
                    char c = netCode.charAt(i);
                    if (!Character.isLetterOrDigit(c)) {
                        throw new IllegalArgumentException("Code has grammar error: " + currentBuffer);
                    } else if (currentBuffer.length() != 0){
                        currentBuffer.append(c);
                    }
            }
            if (currentLevel < 0) {
                throw new IllegalArgumentException("Bracket is not pair: " + currentBuffer);
            }
        }

        if (currentLevel > 0) {
            throw new IllegalArgumentException("Still have bracket is not paired");
        }
        return this;
    }

    private Parser check() {
        // Process define
        if (tokenList.get(0).content() == "("
                && tokenList.get(1).content().toLowerCase() == "define"
                && tokenList.get(2).type() == TokenType.word) {
            name = tokenList.get(2).content();
            if (tokenList.get(3).type() != TokenType.leftBracket
                    && tokenList.get(3).type() != TokenType.leftSquareBracket) {
                throw new IllegalArgumentException("After define name, it should be '(' or '[' there");
            }
        }
        // TODO check detail about pair is [], not like [)

        // check word place
        TokenType lastType = null, currentType = null, nextType = tokenList.get(3).type();
        for (int i = 3; i < tokenList.size(); ++i) {
            currentType = nextType;
            nextType = tokenList.get(i + 1).type();
            if (currentType == TokenType.word) {
                if (lastType != TokenType.leftBracket || nextType != TokenType.rightBracket) {
                    throw new IllegalArgumentException("The word can only be allowed between ( )");
                }
            }
            lastType = currentType;
        }
        return this;
    }

    private Node doParse(int i) {
        int pairedOne;
        switch (tokenList.get(i).type()) {
            case leftBracket:
                pairedOne = findPair(i, TokenType.rightBracket);
                return processBracket(i, pairedOne);
            case leftSquareBracket:
                pairedOne = findPair(i, TokenType.rightSquareBracket);
                return processSquareBracket(i, pairedOne);
            default:
                // TODO should declare NAME in the document
                throw new IllegalArgumentException("Found the illegal element after the NAME");
        }

    }

    private int findPair(int i, TokenType destType) {
        int srcLevel = tokenList.get(i).level();
        for (; i < tokenList.size(); ++i) {
            if (tokenList.get(i).type() == destType && tokenList.get(i).level() == srcLevel) {
                return i;
            }
        }
        throw new IllegalArgumentException("Not found the paired one of " + tokenList.get(i).content());
    }

    private int findPair(int i) {
        switch (tokenList.get(i).type()) {
            case leftBracket:
                return findPair(i, TokenType.rightBracket);
            case leftSquareBracket:
                return findPair(i, TokenType.rightSquareBracket);
            default:
                throw new IllegalArgumentException("Not found the paired one of " + tokenList.get(i).content());
        }
    }

    private Node processBracket(int begin, int end) {
        switch (end - begin) {
            case 1: // () should in grammar check? TODO
                throw new IllegalArgumentException("The content between two paired bracket should not be empty");
            case 2: // (word)
                return new Node(tokenList.get(begin + 1).content());
            default: // like (()[])
                Node last;
                Node first = last = doParse(begin + 1);
                begin = findPair(begin + 1);

                while (begin < end) {
                    // there are some info lose between the two function
                    Node n = doParse(begin + 1);
                    last.addNode(n);
                    // update
                    last = n;
                    begin = findPair(begin + 1); // actually begin point to the last pair's end
                }
                return first; // or should return last
        }
    }

    private Node processSquareBracket(int begin, int end) {
        switch (end - begin) {
            case 1: // [] should in grammar check? TODO
                throw new IllegalArgumentException("The content between two paired bracket should not be empty");
            case 2: // [word]
                return new Node(tokenList.get(begin + 1).content());
            default: // like [()[]]
                Node n = new Node("");
                while (begin < end) {
                    n.addNode(doParse(begin + 1));
                    begin = findPair(begin + 1); // actually begin point to the last pair's end
                }
                return n;
        }
    }
}
