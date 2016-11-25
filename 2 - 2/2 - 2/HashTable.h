#ifndef __HashTable
#define __HashTable

template<typename T> class HashTable_link {
private:
	class Node {
	public :
		T data;
		Node *next;
		Node(T &d) : data(d), next(nullptr) {};
	};
	Node **table;
	int (*hash)(T);
	int s;
public:
	HashTable_link(int size, int(*hashfunc)(T)) : s(size), hash(hashfunc) {
		table = new Node*[s];
		memset(table, 0, s * sizeof(Node*));
	};
	~HashTable_link() {
		for (int i = 0;i < s;i++) {
			Node *now = *(table + i);
			while (now != nullptr) {
				Node *t = now->next;
				delete now;
				now = t;
			}
		}
		delete table;
	};
	void add(T &d) {
		int key = hash(d) % s;
		Node *now = *(table + key);
		if (now == nullptr) {
			*(table + key) = new Node(d);
			return;
		}
		while (now->next != nullptr)
			now = now->next;
		now->next = new Node(d);
	};
	void remove(T &d) {
		int key = hash(d) % s;
		Node *now = *(table + key);
		if (now == nullptr)
			throw exception("HashTable_link : data is not found.");
		if (now->next == nullptr) {
			if (now->data == d) {
				delete now;
				*(table + key) = nullptr;
				return;
			}
			throw exception("HashTable_link : data is not found.");
		}
		while (now->next != nullptr) {
			if (now->next->data == d) {
				Node *t = now->next->next;
				delete now->next;
				now->next = t;
				return;
			}
			now = now->next;
		}
		throw exception("HashTable_link : data is not found.");
	};
	bool exist(T &d) {
		int key = hash(d) % s;
		Node *now = *(table + key);
		if (now == nullptr) return false;
		while (now != nullptr) {
			if (now->data == d) return true;
			now = now->next;
		}
		return false;
	};
};

template<typename T> class HashTable_nonlink {
private:
	class Node {
	public:
		T data;
		bool flag;
		Node(T &d) : data(d), flag(true) {};
	};
	Node **table;
	int (*hash)(T, int);
	int s, cnt;
public:
	HashTable_nonlink(int size, int(*hashfunc)(T, int)) : cnt(0), s(size), hash(hashfunc) {
		table = new Node*[s];
		memset(table, 0, s * sizeof(Node*));
	};
	~HashTable_nonlink() {
		for (int i = 0;i < s;i++) {
			Node *now = *(table + i);
			if (now == nullptr) delete now;
		}
		delete table;
	};
	void add(T &d) {
		if (cnt > s / 2) throw exception("HashTable_nonlink : size is too small.");
		for (int i = 0;;i++) {
			int key = hash(d, i) % s;
			Node *now = *(table + key);
			if (now == nullptr) {
				cnt++;
				*(table + key) = new Node(d);
				break;
			} else if (now->flag == false) {
				now->flag = true;
				now->data = d;
				break;
			}
		}
	};
	void remove(T &d) {
		for (int i = 0;;i++) {
			int key = hash(d, i) % s;
			Node *now = *(table + key);
			if (now->flag == true && now->data == d) {
				now->flag = false;
				return;
			}
			if (now == nullptr) throw exception("HashTable_nonlink : data is not found.");
		}
	};
	bool exist(T &d) {
		for (int i = 0;;i++) {
			int key = hash(d, i) % s;
			Node *now = *(table + key);
			if (now == nullptr) return false;
			if (now->flag == true && now->data == d) return true;
		}
	}
};

#endif