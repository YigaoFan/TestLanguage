package com.company;

/**
 * Created by LaoFan on 3/29/19.
 */
public class Tree {
    private String name;
    private Node headNode;

    public Tree(String name, Node headNode) {
        this.name = name;
        this.headNode = headNode;
    }

    public String name() {
        return name;
    }

    public Node headNode() {
        return headNode;
    }
}
