package com.company;

import java.io.BufferedReader;
import java.io.FileReader;

public class Main {

    public static void main(String[] args) {
        StringBuffer code = new StringBuffer();
        try {
            BufferedReader in = new BufferedReader(new FileReader("/Users/LaoFan/configure.scm"));
            String str;
            while ((str = in.readLine()) != null) {
                code.append(str);
            }
        } catch (Exception e) {
            System.out.print("Read file failed");
        }

        Tree t = Parser.parse(code.toString());
        Node n = t.headNode();
    }
}
