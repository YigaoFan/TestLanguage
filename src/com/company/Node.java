package com.company;

import java.util.ArrayList;
import java.util.Iterator;

/**
 * Created by LaoFan on 3/29/19.
 */
public class Node implements Iterable {
    private String content; // content could be ""
    private ArrayList<Node> nodeChilds;

    public Node(String content) {
        this.content = content;
        nodeChilds = new ArrayList<Node>();
    }

    public String content() {
        return content;
    }

    public Node addNode(Node node) {
        nodeChilds.add(node);
        return this;
    }

    public Node get(int i) {
        return nodeChilds.get(i);
    }

    public int childCount() {
        return nodeChilds.size();
    }

    public Iterator iterator() {
        return new Itr();
    }

    private class Itr implements Iterator {
        private int next = 0;

        public boolean hasNext() {
            return next != childCount();
        }

        public Node next() {
            return get(next++);
        }
    }

}
