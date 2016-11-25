#include<iostream>
#include "DoubleList.h"
#include "HashTable.h"
using namespace std;

void testDoublyLinkedList() {
	int i;
	DoubleList<int> list = DoubleList<int>();
	for (i = 0; i < 10; i++)
		list.add(i);
	for (i = 4;i < 7;i++)
		list.remove(i);
	for (i = 0; i < 7; i++)
		cout << list.get(i) << endl;
}
int hasha(int i) { return i; }
int hashb(int a, int b) { return a + b * b; }
void testHashTable() {
	HashTable_link<int> ht = HashTable_link<int>(10,hasha);
	int i = 5;
	ht.add(i);ht.add(i);ht.add(i);
	ht.remove(i);ht.remove(i);ht.remove(i);
	if (ht.exist(i))
		cout << i << " exist." << endl;
	else
		cout << i << " not exist." << endl;
	i = 3;
	ht.add(i);ht.add(i);ht.add(i);
	ht.remove(i);ht.remove(i);
	if (ht.exist(i))
		cout << i << " exist." << endl;
	else
		cout << i << " not exist." << endl;

	HashTable_nonlink<int> tht = HashTable_nonlink<int>(10, hashb);
	i = 5;
	tht.add(i);tht.add(i);tht.add(i);
	tht.remove(i);tht.remove(i);tht.remove(i);
	if (tht.exist(i))
		cout << i << " exist." << endl;
	else
		cout << i << " not exist." << endl;
	i = 3;
	tht.add(i);tht.add(i);tht.add(i);
	tht.remove(i);tht.remove(i);
	if (tht.exist(i))
		cout << i << " exist." << endl;
	else
		cout << i << " not exist." << endl;
}

int main() {
	try {
		testDoublyLinkedList();
		testHashTable();
	} catch(exception &e) {
		cout << e.what() << endl;
	}
	return 0;
}