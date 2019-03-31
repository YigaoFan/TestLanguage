package com.company;

/**
 * Created by LaoFan on 3/30/19.
 */
enum TokenType {
    word,
    leftBracket,
    rightBracket,
    leftSquareBracket,
    rightSquareBracket,
}

public class Token {
    private String content;
    private int level;
    private TokenType type;

    public Token(String content, int level, TokenType type) {
        this.content = content;
        this.level = level;
        this.type = type;
    }

    public String content() {
        return content;
    }

    public int level() {
        return level;
    }

    public TokenType type() {
        return type;
    }
}
