#ifndef __DoubleList
#define __DoubleList

template<typename T> class DoubleList {
private:
	class Node {
	public:
		T data;
		Node *front, *back;
		Node(T &d) : data(d),front(nullptr),back(nullptr) {}
	};
	Node *head, *tail;
	int cnt;
public:
	DoubleList() : cnt(0), head(nullptr), tail(nullptr) {};
	~DoubleList() {
		Node *now = head;
		while (now != nullptr) {
			Node *t = now->back;
			delete now;
			now = t;
		}
	};
	void add(T &d) {
		cnt++;
		if (head == nullptr) {
			head = tail = new Node(d);
			return;
		}
		Node *n = new Node(d);
		tail->back = n;
		n->front = tail;
		tail = n;
	};
	void remove(T &d) {
		cnt--;
		Node *now;
		if (head->data == d) {
			now = head->back;
			delete head;
			if (now != nullptr) {
				head = now;
				head->front = nullptr;
			}
			return;
		}
		if (tail->data == d) {
			now = tail->front;
			delete tail;
			if (now != nullptr) {
				tail = now;
				tail->back = nullptr;
			}
			return;
		}
		now = head->back;
		while (now != tail) {
			if (now->data == d) {
				now->back->front = now->front;
				now->front->back = now->back;
				delete now;
				return;
			}
			now = now->back;
		}
		throw exception("DoubleList : data is not found.");
	};
	T get(int k) {
		if (k < 0 || k >= cnt) throw exception("DoubleList : k is out of range.");
		Node *now = head;
		for (int i = 0;i < k;i++)
			now = now->back;
		return now->data;
	};
};

#endif