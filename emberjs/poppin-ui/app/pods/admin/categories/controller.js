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

	populateForm(cat) {
		set(this, 'cleanCategory', cat);
		set(this, 'id', cat.id);
		set(this, 'name', cat.name);
		set(this, 'slug', cat.slug);
		set(this, 'parent', cat.parent);
		set(this, 'hereId', cat.hereId);
		set(this, 'relatedCSV', (cat.related || []).join(','));
		set(this, 'childrenCSV', (cat.children || []).join(','));
	}

	@action
	addOrUpdateCategory() {
		let method = this.isUpdate ? 'updateCategory' : 'addCategory';
		const category = {
			id: this.id,
			name: this.name,
			slug: this.slug,
			parent: this.parent,
			hereId: this.hereId,
			related: this.relatedCSV ? this.relatedCSV.split(',').map(r => r.trim()) : null,
			children: this.childrenCSV ? this.childrenCSV.split(',').map(r => r.trim()) : null
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
