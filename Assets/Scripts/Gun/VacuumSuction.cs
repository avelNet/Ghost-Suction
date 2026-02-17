using UnityEngine;

public class VacuumSuction : MonoBehaviour
{
    [Header("Settings")]
    public Transform suctionPoint;    // Точка-дуло
    public float suctionRange = 10f;  // Радиус действия
    public float suctionForce = 20f;  // Сила всасывания
    public float stopDistance = 0.8f; // Дистанция "падения" мусора
    public LineRenderer vacuumLine;
    private Rigidbody _capturedRB; // Объект, который мы сейчас тянем
    private Vector3 _hitOffset;

    [Header("Input")]
    public KeyCode suctionKey = KeyCode.Mouse0; // ЛКМ по умолчанию

    void Update()
    {
        if (Input.GetKey(suctionKey))
        {
            UpdateVacuumLine();
        }
        else
        {
            vacuumLine.enabled = false;
            _capturedRB = null; // Отпускаем объект, если кнопка отпущена
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(suctionKey))
        {
            ApplySuction();
        }
    }

    private void UpdateVacuumLine()
    {
        vacuumLine.enabled = true;
        vacuumLine.SetPosition(0, suctionPoint.position);

        // Если мы ЕЩЕ НИКОГО НЕ ПОЙМАЛИ
        if (_capturedRB == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(suctionPoint.position, suctionPoint.forward, out hit, suctionRange))
            {
                // Рисуем луч до точки попадания
                vacuumLine.SetPosition(1, hit.point);

                // Если у объекта есть Rigidbody — захватываем его!
                if (hit.rigidbody != null)
                {
                    _capturedRB = hit.rigidbody;
                    // Запоминаем, куда именно мы попали в теле объекта
                    _hitOffset = _capturedRB.transform.InverseTransformPoint(hit.point);
                }
            }
            else
            {
                // Если никуда не попали — луч просто летит вперед как "пуля"
                vacuumLine.SetPosition(1, suctionPoint.position + (suctionPoint.forward * suctionRange));
            }
        }
        // Если МЫ УЖЕ ПОЙМАЛИ ОБЪЕКТ
        else
        {
            // Конец луча всегда следует за объектом (в ту самую точку попадания)
            Vector3 currentHitPoint = _capturedRB.transform.TransformPoint(_hitOffset);
            vacuumLine.SetPosition(1, currentHitPoint);

            // Прикладываем силу к объекту, чтобы он тянулся к нам
            ApplyForceToCaptured(currentHitPoint);

            // Если объект улетел слишком далеко (сорвался с крючка)
            if (Vector3.Distance(suctionPoint.position, currentHitPoint) > suctionRange + 2f)
            {
                _capturedRB = null;
            }
        }
    }

    private void ApplyForceToCaptured(Vector3 targetPoint)
    {
        Vector3 direction = suctionPoint.position - targetPoint;
        float distance = direction.magnitude;

        // Проверка на "мелкий мусор" (то, что ты хотел раньше)
        bool isGhost = _capturedRB.gameObject.layer == LayerMask.NameToLayer("Ghost");

        if (isGhost || distance > stopDistance)
        {
            _capturedRB.AddForce(direction.normalized * suctionForce, ForceMode.Acceleration);
        }
        else
        {
            // Тормозим предмет у дула
            _capturedRB.linearVelocity *= 0.5f;
        }
    }

    private void ApplySuction()
    {
        // Ищем все коллайдеры в радиусе
        Collider[] colliders = Physics.OverlapSphere(suctionPoint.position, suctionRange);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            // Тянем только то, у чего есть физика (Rigidbody)
            if (rb != null)
            {
                Vector3 direction = suctionPoint.position - hit.transform.position;
                float distance = direction.magnitude;

                // Условие разделения мусора и призраков по слоям
                bool isGhost = hit.gameObject.layer == LayerMask.NameToLayer("Ghost");

                if (isGhost)
                {
                    // Призраков тянем всегда сильно и до конца
                    rb.AddForce(direction.normalized * suctionForce * 1.5f);
                }
                else
                {
                    // Мелкий мусор: если слишком близко — перестаем тянуть
                    if (distance > stopDistance)
                    {
                        rb.AddForce(direction.normalized * suctionForce);
                    }
                    else
                    {
                        // Эффект "выпадения": гасим скорость, чтобы не липло к дулу
                        rb.linearVelocity *= 0.9f;
                    }
                }
            }
        }
    }

    // Отрисовка радиуса в редакторе для удобства
    private void OnDrawGizmosSelected()
    {
        if (suctionPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(suctionPoint.position, suctionRange);
        }
    }
}