/* eslint-disable no-case-declarations */
import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';

export default class AdminCategoriesController extends Controller {
	@service categoriesService;
	@tracked isUpdate;

	@tracked cleanCategory;
	@tracked name;
	@tracked slug;
	@tracked parent;
	@tracked hereId;
	@tracked relatedCSV;
	@tracked childrenCSV

	get categories() {
		return this.categoriesService.categories;
	}

	populateForm(cat) {
		const parCatArr = this.categories.filter(c => c.slug == cat.parent);
		set(this, 'cleanCategory', cat);
		set(this, 'id', cat.id);
		set(this, 'name', cat.name);
		set(this, 'slug', cat.slug);
		set(this, 'parent', parCatArr.length ? parCatArr[0] : null);
		set(this, 'hereId', cat.hereId);
		set(this, 'related', this.categories.filter(c => (cat.related || []).indexOf(c.slug) > -1) || []);
		set(this, 'children', this.categories.filter(c => (cat.children || []).indexOf(c.slug) > -1) || []);
	}

	@action
	addOrUpdateCategory() {
		let method = this.isUpdate ? 'updateCategory' : 'addCategory';
		const category = {
			id: this.id,
			name: this.name,
			slug: this.slug,
			parent: this.parent ? this.parent.slug : null,
			hereId: this.hereId,
			related: (this.related || []).map(r => r.slug),
			children: (this.children || []).map(c => c.slug)
		}
		const params = {
			category,
			original: this.cleanCategory
		};

		return this.categoriesService[method](this.isUpdate ? params : category).then(() => {
			this.clearForm();
			this.send('reload');
		}).catch(e => console.error(e));
	}

	@action
	clickAction(category) {
		this.isUpdate = true;
		this.populateForm(category);
	}

	@action
	clearForm() {
		this.isUpdate = false;
		this.populateForm({});
	}
}
